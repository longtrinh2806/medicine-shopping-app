package com.trinhkimlong.authentication.views;

import lombok.AllArgsConstructor;
import lombok.Builder;
import lombok.Data;
import lombok.NoArgsConstructor;

@Data
@AllArgsConstructor
@NoArgsConstructor
@Builder
public class ResponseModel {
    private Object Data;
    private boolean Succeeded;
    private String Message;
}
