import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { LoginComponent } from './pages/login/login.component';
import { UserSearchComponent } from './pages/user-search/user-search.component';
import { RegisterComponent } from './pages/register/register.component';
import { UserMessagesComponent } from './pages/user-messages/user-messages.component';

const routes: Routes = [
  { path: 'login', component: LoginComponent },
  { path: 'register', component: RegisterComponent },
  { path: 'user-search', component: UserSearchComponent },
  { path: 'user-messages', component: UserMessagesComponent },
  { path: '', redirectTo: '/login', pathMatch: 'full' },
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule],
})
export class AppRoutingModule {}
