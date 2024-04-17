package com.trinhkimlong.authentication.controllers;

import com.trinhkimlong.authentication.dtos.UserDto;
import com.trinhkimlong.authentication.services.IAuthenticationService;
import com.trinhkimlong.authentication.views.ResponseModel;
import jakarta.servlet.http.HttpServletRequest;
import jakarta.servlet.http.HttpServletResponse;
import lombok.RequiredArgsConstructor;
import org.springframework.http.ResponseEntity;
import org.springframework.web.bind.annotation.PostMapping;
import org.springframework.web.bind.annotation.RequestBody;
import org.springframework.web.bind.annotation.RequestMapping;
import org.springframework.web.bind.annotation.RestController;

import java.io.IOException;

@RestController
@RequestMapping("api/auth")
@RequiredArgsConstructor
public class AuthenticationController {

    private final IAuthenticationService _authService;

    @PostMapping("/register")
    public ResponseEntity<ResponseModel> Register(@RequestBody UserDto request) {
        var result = _authService.Register(request);

        if (result.isSucceeded())
            return ResponseEntity.ok(result);
        return ResponseEntity.badRequest().body(result);
    }

    @PostMapping("/login")
    public ResponseEntity<ResponseModel> LogIn(@RequestBody UserDto request) {
        var result = _authService.LogIn(request);

        if (result.isSucceeded())
            return ResponseEntity.ok(result);
        return ResponseEntity.badRequest().body(result);
    }

    @PostMapping("/refresh-token")
    public void RefreshToken(
            HttpServletRequest request,
            HttpServletResponse response) throws IOException {
        _authService.RefreshToken(request, response);
    }
}
