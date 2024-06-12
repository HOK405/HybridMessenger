import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { ReactiveFormsModule } from '@angular/forms';
import { HttpClientModule } from '@angular/common/http';

import { AppComponent } from './app.component';
import { LoginComponent } from './Pages/login/login.component';
import { AppRoutingModule } from './app-routing.module';
import { LoggedInComponent } from './Pages/logged-in/logged-in.component';
import { UserSearchComponent } from './Pages/user-search/user-search.component';
import { RegisterComponent } from './Pages/register/register.component';

@NgModule({
  declarations: [
    AppComponent,
    LoginComponent,
    LoggedInComponent,
    UserSearchComponent,
    RegisterComponent,
  ],
  imports: [
    BrowserModule,
    ReactiveFormsModule,
    HttpClientModule,
    AppRoutingModule,
  ],
  bootstrap: [AppComponent],
})
export class AppModule {}
