import { Injectable } from '@angular/core';
import { BehaviorSubject } from 'rxjs';

@Injectable({
    providedIn: 'root'
})
export class AuthService {
    private _isLoggedIn = new BehaviorSubject<boolean>(this.hasToken());
    private _userRole = new BehaviorSubject<string | null>(this.getRole());

    isLoggedIn$ = this._isLoggedIn.asObservable();
    userRole$ = this._userRole.asObservable();

    getRoleFromToken(token: string): string | null {
        try {
            const payload = JSON.parse(atob(token.split('.')[1]));
            return payload.role || payload["http://schemas.microsoft.com/ws/2008/06/identity/claims/role"] || null;
        } catch (e) {
            return null;
        }
    }

    login(token: string) {
        localStorage.setItem('token', token);
        const role = this.getRoleFromToken(token);
        if (role) {
            localStorage.setItem('role', role);
            this._userRole.next(role);
        } else {
            localStorage.removeItem('role');
            this._userRole.next(null);
        }
        this._isLoggedIn.next(true);
    }

    logout() {
        localStorage.removeItem('token');
        localStorage.removeItem('role');
        this._isLoggedIn.next(false);
        this._userRole.next(null);
    }

    hasToken(): boolean {
        return !!localStorage.getItem('token');
    }

    getRole(): string | null {
        return localStorage.getItem('role');
    }

    isTokenExpired(token: string): boolean {
        try {
            const payload = JSON.parse(atob(token.split('.')[1]));
            const now = Math.floor(Date.now() / 1000);
            return payload.exp && payload.exp < now;
        } catch (e) {
            return true;
        }
    }

}
