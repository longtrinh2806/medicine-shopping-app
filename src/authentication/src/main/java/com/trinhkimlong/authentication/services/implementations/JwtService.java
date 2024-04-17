package com.trinhkimlong.authentication.services.implementations;

import com.trinhkimlong.authentication.services.IJwtService;
import io.jsonwebtoken.Claims;
import io.jsonwebtoken.Jwts;
import io.jsonwebtoken.SignatureAlgorithm;
import io.jsonwebtoken.io.Decoders;
import io.jsonwebtoken.security.Keys;
import org.springframework.beans.factory.annotation.Value;
import org.springframework.security.core.userdetails.UserDetails;
import org.springframework.stereotype.Service;

import java.security.Key;
import java.util.Date;
import java.util.HashMap;
import java.util.Map;
import java.util.function.Function;

@Service
public class JwtService implements IJwtService {
    @Value("${application.security.jwt.secret-key}")
    private String SECRET_KEY;
    private static final long accessTokenExpiration = 1000 * 60 * 5; // 10 minutes

    private static final long MILLISECONDS_PER_DAY = 1000 * 60 * 60 * 24;
    private static final long refreshTokenExpiration = MILLISECONDS_PER_DAY * 30; // 30 days
    @Override
    public String ExtractEmail(String token) {
        return ExtractClaim(token, Claims::getSubject);
    }

    public <T> T ExtractClaim(String token, Function<Claims, T> claimsResolver) {
        final Claims claims = ExtractAllClaims(token);
        return claimsResolver.apply(claims);
    }

    public String GenerateAccessToken(UserDetails userDetails) {
        return GenerateAccessToken(new HashMap<>(), userDetails);
    }

    public String GenerateAccessToken(Map<String, Object> extraClaims, UserDetails userDetails) {
        return BuildToken(extraClaims, userDetails, accessTokenExpiration);
    }

    public String GenerateRefreshToken(UserDetails userDetails) {
        return BuildToken(new HashMap<>(), userDetails, refreshTokenExpiration);
    }

    private String BuildToken(Map<String, Object> extraClaims, UserDetails userDetails, long expiration) {
        return Jwts
                .builder()
                .setClaims(extraClaims)
                .setSubject(userDetails.getUsername())
                .setIssuedAt(new Date(System.currentTimeMillis()))
                .setExpiration(new Date(System.currentTimeMillis() + expiration))
                .signWith(getSignInKey(), SignatureAlgorithm.HS256)
                .compact();
    }

    public boolean IsTokenValid(String token, UserDetails userDetails) {
        final String email = ExtractEmail(token);
        return (email.equals(userDetails.getUsername()) && !IsTokenExpired(token));
    }

    private Claims ExtractAllClaims(String token) {
        return Jwts
                .parserBuilder()
                .setSigningKey(getSignInKey())
                .build()
                .parseClaimsJws(token)
                .getBody();
    }

    private boolean IsTokenExpired(String token) {
        return ExtractExpiration(token).before(new Date());
    }

    private Date ExtractExpiration(String token) {
        return ExtractClaim(token, Claims::getExpiration);
    }

    private Key getSignInKey() {
        byte[] keyBytes = Decoders.BASE64.decode(SECRET_KEY);
        return Keys.hmacShaKeyFor(keyBytes);
    }
}
