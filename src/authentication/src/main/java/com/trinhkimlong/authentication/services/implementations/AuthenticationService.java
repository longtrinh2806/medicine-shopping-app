package com.trinhkimlong.authentication.services.implementations;

import com.fasterxml.jackson.databind.ObjectMapper;
import com.trinhkimlong.authentication.contracts.UserCreated;
import com.trinhkimlong.authentication.dtos.AuthResponse;
import com.trinhkimlong.authentication.dtos.UserDto;
import com.trinhkimlong.authentication.models.Role;
import com.trinhkimlong.authentication.models.User;
import com.trinhkimlong.authentication.publish.RabbitMQProducer;
import com.trinhkimlong.authentication.repositories.IUserRepository;
import com.trinhkimlong.authentication.services.IAuthenticationService;
import com.trinhkimlong.authentication.services.IJwtService;
import com.trinhkimlong.authentication.views.ResponseModel;
import jakarta.servlet.http.HttpServletRequest;
import jakarta.servlet.http.HttpServletResponse;
import lombok.RequiredArgsConstructor;
import org.springframework.amqp.rabbit.core.RabbitTemplate;
import org.springframework.http.HttpHeaders;
import org.springframework.security.authentication.AuthenticationManager;
import org.springframework.security.authentication.UsernamePasswordAuthenticationToken;
import org.springframework.security.core.context.SecurityContextHolder;
import org.springframework.security.core.userdetails.UserDetails;
import org.springframework.security.crypto.password.PasswordEncoder;
import org.springframework.security.web.authentication.WebAuthenticationDetailsSource;
import org.springframework.stereotype.Service;

import java.io.IOException;
import java.util.HashMap;
import java.util.Map;

@Service
@RequiredArgsConstructor
public class AuthenticationService implements IAuthenticationService {
    private final IUserRepository _userRepo;
    private final PasswordEncoder _passwordEncoder;
    private final IJwtService _jwtService;
    private final AuthenticationManager _authenticationManager;
    private final RabbitMQProducer _producer;

    @Override
    public ResponseModel Register(UserDto request) {
        try {
            var user = User
                    .builder()
                    .email(request.getEmail())
                    .password(_passwordEncoder.encode(request.getPassword()))
                    .role(Role.user)
                    .build();
            _userRepo.save(user);

            _producer.sendJsonMessage(new UserCreated(user.getId(), user.getEmail()));

            return ResponseModel
                    .builder()
                    .Succeeded(true)
                    .Message("Register Successfully")
                    .build();
        }
        catch (Exception ex) {
            return ResponseModel
                    .builder()
                    .Succeeded(false)
                    .Message(ex.getMessage())
                    .build();
        }
    }

    @Override
    public ResponseModel LogIn(UserDto request) {
        try {
            _authenticationManager.authenticate(
                    new UsernamePasswordAuthenticationToken(request.getEmail(), request.getPassword()));

            var user = _userRepo.findByEmail(request.getEmail()).orElseThrow();

            Map<String, Object> claims = new HashMap();
            claims.put("email", user.getEmail());
            claims.put("role", user.getRole());

            var accessToken = _jwtService.GenerateAccessToken(claims, user);
            var refreshToken = _jwtService.GenerateRefreshToken(user);

            var authResponse = new AuthResponse(accessToken, refreshToken);

            return ResponseModel
                    .builder()
                    .Data(authResponse)
                    .Succeeded(true)
                    .Message("Sign In Successfully")
                    .build();
        }
        catch (Exception ex) {
            return ResponseModel
                    .builder()
                    .Succeeded(false)
                    .Message(ex.getMessage())
                    .build();
        }
    }

    @Override
    public void RefreshToken(HttpServletRequest request, HttpServletResponse response) throws IOException {
        final String authHeader = request.getHeader(HttpHeaders.AUTHORIZATION);
        final String refreshToken;
        final String email;

        if (authHeader == null || !authHeader.startsWith("Bearer ")) {
            return;
        }

        refreshToken = authHeader.substring(7);
        email = _jwtService.ExtractEmail(refreshToken);

        if (email != null) {
            var user = _userRepo.findByEmail(email).orElseThrow();

            if (_jwtService.IsTokenValid(refreshToken, user)) {

                var newAccessToken = _jwtService.GenerateAccessToken(user);
                var authResponse = new AuthResponse(newAccessToken, refreshToken);

                new ObjectMapper().writeValue(response.getOutputStream(), authResponse);
            }
        }
    }
}
