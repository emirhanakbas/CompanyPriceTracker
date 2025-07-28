import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import {OfferComponent} from './pages/offer/offer.component';
import {LoginComponent} from './pages/login/login.component';
import {AdminRegisterComponent} from './pages/admin-register/admin-register.component';
import {AddCompanyComponent} from './pages/add-company/add-company.component';
import {AuthGuard} from "./guards/auth.guard";

const routes: Routes = [
  {path: '', component: OfferComponent},
  {path: 'login', component: LoginComponent},
  {path: 'admin-register', component: AdminRegisterComponent, canActivate: [AuthGuard], data: { role: 'Admin' }},
  {path: 'add-company', component: AddCompanyComponent, canActivate: [AuthGuard], data: { role: ['Admin', 'User'] }},
  {path: '**', redirectTo: ''},
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
