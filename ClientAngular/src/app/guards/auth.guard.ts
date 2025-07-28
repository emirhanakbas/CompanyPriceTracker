// src/app/guards/auth.guard.ts
import { Injectable } from '@angular/core';
import { CanActivate, ActivatedRouteSnapshot, RouterStateSnapshot, Router, UrlTree } from '@angular/router';
import { Observable } from 'rxjs';
import { AuthService } from '../services/auth.service';

@Injectable({
    providedIn: 'root'
})
export class AuthGuard implements CanActivate {

    constructor(private authService: AuthService, private router: Router) {}

    canActivate(
        route: ActivatedRouteSnapshot,
        state: RouterStateSnapshot
    ): boolean | UrlTree {
        const isLoggedIn = this.authService.hasToken();
        const userRole = this.authService.getRole();

        const expectedRole = route.data['role'];
        if (!isLoggedIn) {
            return this.router.parseUrl('/login');
        }

        if (Array.isArray(expectedRole)) {
            if (!expectedRole.includes(userRole)) {
                return this.router.parseUrl('/');
            }
        } else if (expectedRole && userRole !== expectedRole) {
            return this.router.parseUrl('/');
        }

        return true;
    }
}
