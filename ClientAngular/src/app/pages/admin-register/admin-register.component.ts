// src/app/pages/admin-register/admin-register.component.ts
import { Component } from '@angular/core';
import { FormBuilder, Validators } from '@angular/forms';
import { HttpClient, HttpErrorResponse } from '@angular/common/http';

@Component({
    selector: 'app-admin-register',
    templateUrl: './admin-register.component.html'
})
export class AdminRegisterComponent {
    successMsg = '';
    errorMsg = '';
    validationErrors: any = {};

    registerForm = this.fb.group({
        // Kullanıcı adı için yeni validasyon
        username: ['', [
            Validators.required,
            Validators.minLength(3),
            Validators.pattern('^[a-zA-Z0-9_]*$') // Sadece harf, sayı ve _ içerebilir
        ]],
        // Şifre için yeni validasyon
        password: ['', [
            Validators.required,
            Validators.minLength(6),
            Validators.pattern('^(?=.*[a-z])(?=.*[A-Z])(?=.*[0-9])(?=.*[!@#$%^&*])(?=.{6,})') // Büyük/küçük harf, sayı ve özel karakter içermeli
        ]]
    });

    constructor(private fb: FormBuilder, private http: HttpClient) {}

    onSubmit() {
        if (this.registerForm.invalid) return;

        this.validationErrors = {};

        this.http.post<any>('http://localhost:5000/api/auth/register', this.registerForm.value)
            .subscribe({
                next: res => {
                    this.successMsg = 'Firma yetkilisi başarıyla kaydedildi!';
                    this.registerForm.reset();
                    this.errorMsg = '';
                },
                error: (err: HttpErrorResponse) => {
                    this.successMsg = '';
                    if (err.error && err.error.errors) {
                        this.validationErrors = err.error.errors;
                    } else if (err.error && err.error.message) {
                        this.errorMsg = err.error.message;
                    } else {
                        this.errorMsg = 'Kayıt başarısız oldu. Lütfen tekrar deneyin.';
                    }
                }
            });
    }
}
