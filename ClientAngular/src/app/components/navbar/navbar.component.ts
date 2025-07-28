import { Component, OnInit } from '@angular/core';
import { AuthService } from '../../services/auth.service';

@Component({
    selector: 'app-navbar',
    templateUrl: './navbar.component.html',
})
export class NavbarComponent implements OnInit {
    userRole: string | null = null;
    isLoggedIn = false;

    constructor(private authService: AuthService) {}

    ngOnInit() {
        this.authService.userRole$.subscribe(role => this.userRole = role);
        this.authService.isLoggedIn$.subscribe(isLogged => this.isLoggedIn = isLogged);
    }

    logout() {
        this.authService.logout();
    }
}
