package com.trinhkimlong.authentication.services;

import io.jsonwebtoken.Claims;
import org.springframework.security.core.userdetails.UserDetails;

import java.util.Map;
import java.util.function.Function;

public interface IJwtService {
    String ExtractEmail(String token);
    <T> T ExtractClaim(String token, Function<Claims, T> claimsResolver);
    String GenerateAccessToken(Map<String, Object> extraClaims, UserDetails userDetails);
    String GenerateAccessToken(UserDetails userDetails);
    String GenerateRefreshToken(UserDetails userDetails);
    boolean IsTokenValid(String token, UserDetails userDetails);
}
