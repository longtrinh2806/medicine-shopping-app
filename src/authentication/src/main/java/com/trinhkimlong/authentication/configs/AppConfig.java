package com.trinhkimlong.authentication.configs;

import com.trinhkimlong.authentication.contracts.UserCreated;
import com.trinhkimlong.authentication.models.Role;
import com.trinhkimlong.authentication.models.User;
import com.trinhkimlong.authentication.publish.RabbitMQProducer;
import com.trinhkimlong.authentication.repositories.IUserRepository;
import jakarta.annotation.PostConstruct;
import lombok.RequiredArgsConstructor;
import org.springframework.amqp.rabbit.core.RabbitTemplate;
import org.springframework.context.annotation.Bean;
import org.springframework.context.annotation.Configuration;
import org.springframework.security.authentication.AuthenticationManager;
import org.springframework.security.authentication.AuthenticationProvider;
import org.springframework.security.authentication.dao.DaoAuthenticationProvider;
import org.springframework.security.config.annotation.authentication.configuration.AuthenticationConfiguration;
import org.springframework.security.core.userdetails.UserDetails;
import org.springframework.security.core.userdetails.UserDetailsService;
import org.springframework.security.core.userdetails.UsernameNotFoundException;
import org.springframework.security.crypto.bcrypt.BCryptPasswordEncoder;
import org.springframework.security.crypto.password.PasswordEncoder;

@Configuration
@RequiredArgsConstructor
public class AppConfig {
    private final IUserRepository _userRepo;
    private final RabbitMQProducer _producer;

    @Bean
    public User CreateAdminUser() {
        var adminExisting = _userRepo.findByEmail("admin");
        if (adminExisting.isEmpty()) {
            var admin = User
                    .builder()
                    .email("admin")
                    .password(passwordEncoder().encode("admin"))
                    .role(Role.admin)
                    .build();
            _userRepo.save(admin);

            _producer.sendJsonMessage(new UserCreated(admin.getId(), admin.getEmail()));
        }
        return adminExisting.orElse(null);
    }

    @Bean
    public UserDetailsService userDetailsService() {
        return email -> _userRepo
                .findByEmail(email)
                .orElseThrow(() -> new UsernameNotFoundException("User not found"));
    }

    @Bean
    public AuthenticationProvider authenticationProvider() {
        DaoAuthenticationProvider authenticationProvider = new DaoAuthenticationProvider();
        authenticationProvider.setUserDetailsService(userDetailsService());
        authenticationProvider.setPasswordEncoder(passwordEncoder());
        return authenticationProvider;
    }

    @Bean
    public PasswordEncoder passwordEncoder() {
        return new BCryptPasswordEncoder();
    }

    @Bean
    public AuthenticationManager authenticationManager(AuthenticationConfiguration configuration) throws Exception {
        return configuration.getAuthenticationManager();
    }
}
