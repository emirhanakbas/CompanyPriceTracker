import { Component } from '@angular/core';
import { FormBuilder, Validators } from '@angular/forms';
import { AuthService } from '../../services/auth.service';
import { HttpClient } from '@angular/common/http';
import { Router } from '@angular/router';

@Component({
    selector: 'app-login',
    templateUrl: './login.component.html'
})
export class LoginComponent {
    errorMsg = '';
    loading = false;

    loginForm = this.fb.group({
        username: ['', [Validators.required, Validators.minLength(3)]],
        password: ['', [Validators.required, Validators.minLength(6)]]
    });

    constructor(
        private fb: FormBuilder,
        private http: HttpClient,
        private authService: AuthService,
        private router: Router
    ) {}

    onSubmit() {
        if (this.loginForm.invalid) return;

        this.loading = true;
        this.http.post<any>('http://localhost:5000/api/auth/login', this.loginForm.value)
            .subscribe({
                next: res => {
                    this.authService.login(res.data.token);
                    if (res.role === 'Admin') {
                        this.router.navigate(['/admin-register']);
                    } else if (res.role === 'User') {
                        this.router.navigate(['/add-company']);
                    }  else {
                        this.router.navigate(['/']);
                    }
                },
                error: err => {
                    this.errorMsg = err.error?.message || 'Giriş başarısız';
                    this.loading = false;
                }
            });
    }
}
