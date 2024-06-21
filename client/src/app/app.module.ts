import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { ReactiveFormsModule } from '@angular/forms';
import { HttpClientModule } from '@angular/common/http';

import { AppComponent } from './app.component';
import { LoginComponent } from './Pages/login/login.component';
import { AppRoutingModule } from './app-routing.module';
import { UserSearchComponent } from './Pages/user-search/user-search.component';
import { RegisterComponent } from './Pages/register/register.component';
import { UserMessagesComponent } from './Pages/user-messages/user-messages.component';
import { NgMultiSelectDropDownModule } from 'ng-multiselect-dropdown';

@NgModule({
  declarations: [
    AppComponent,
    LoginComponent,
    UserSearchComponent,
    RegisterComponent,
    UserMessagesComponent,
  ],
  imports: [
    BrowserModule,
    ReactiveFormsModule,
    HttpClientModule,
    AppRoutingModule,
    NgMultiSelectDropDownModule,
  ],
  bootstrap: [AppComponent],
})
export class AppModule {}
