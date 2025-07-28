import { NgModule, LOCALE_ID } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { ReactiveFormsModule } from "@angular/forms";
import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { OfferComponent } from './pages/offer/offer.component';
import { LoginComponent } from './pages/login/login.component';
import { AdminRegisterComponent } from './pages/admin-register/admin-register.component';
import { AddCompanyComponent } from './pages/add-company/add-company.component';
import { NavbarComponent } from './components/navbar/navbar.component';
import { RouterModule } from '@angular/router';
import { HttpClientModule} from "@angular/common/http";
import { HTTP_INTERCEPTORS } from '@angular/common/http';
import { AuthInterceptor } from "./services/auth.interceptor.service";
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatDatepickerModule } from '@angular/material/datepicker';
import { MatNativeDateModule } from '@angular/material/core';
import {BrowserAnimationsModule} from "@angular/platform-browser/animations";
import { registerLocaleData } from '@angular/common'
import localeTr from '@angular/common/locales/tr';

registerLocaleData(localeTr, 'tr');

@NgModule({
  declarations: [
    AppComponent,
    OfferComponent,
    LoginComponent,
    AdminRegisterComponent,
    AddCompanyComponent,
    NavbarComponent,
  ],
  imports: [
    BrowserModule,
    AppRoutingModule,
    ReactiveFormsModule,
    RouterModule.forRoot([]),
    HttpClientModule,
    MatFormFieldModule,
    MatInputModule,
    MatDatepickerModule,
    MatNativeDateModule,
    BrowserAnimationsModule
  ],
  providers: [
      { provide: HTTP_INTERCEPTORS, useClass: AuthInterceptor, multi: true },
      { provide: LOCALE_ID, useValue: 'tr-TR' }
  ],
  bootstrap: [AppComponent]
})
export class AppModule { }
