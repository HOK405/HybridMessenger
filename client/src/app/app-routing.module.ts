import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { LoginComponent } from './Pages/login/login.component';
import { LoggedInComponent } from './Pages/logged-in/logged-in.component';
import { UserSearchComponent } from './Pages/user-search/user-search.component';
import { RegisterComponent } from './Pages/register/register.component';
import { UserMessagesComponent } from './Pages/user-messages/user-messages.component';

const routes: Routes = [
  { path: 'login', component: LoginComponent },
  { path: 'register', component: RegisterComponent },
  { path: 'logged-in', component: LoggedInComponent },
  { path: 'user-search', component: UserSearchComponent },
  { path: 'user-messages', component: UserMessagesComponent },
  { path: '', redirectTo: '/login', pathMatch: 'full' },
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule],
})
export class AppRoutingModule {}
