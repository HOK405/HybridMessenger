import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { LoginComponent } from './Pages/login/login.component';
import { LoggedInComponent } from './Pages/logged-in/logged-in.component';
import { UserSearchComponent } from './Pages/user-search/user-search.component';
import { RegisterComponent } from './Pages/register/register.component';

const routes: Routes = [
  { path: 'login', component: LoginComponent },
  { path: 'register', component: RegisterComponent },
  { path: 'logged-in', component: LoggedInComponent },
  { path: 'user-search', component: UserSearchComponent },
  { path: '', redirectTo: '/login', pathMatch: 'full' },
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule],
})
export class AppRoutingModule {}
