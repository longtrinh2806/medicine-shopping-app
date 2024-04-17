package com.trinhkimlong.authentication.services;

import com.trinhkimlong.authentication.dtos.UserDto;
import com.trinhkimlong.authentication.views.ResponseModel;
import jakarta.servlet.http.HttpServletRequest;
import jakarta.servlet.http.HttpServletResponse;

import java.io.IOException;

public interface IAuthenticationService {
    ResponseModel Register(UserDto request);

    ResponseModel LogIn(UserDto request);

    void RefreshToken(HttpServletRequest request, HttpServletResponse response) throws IOException;
}
