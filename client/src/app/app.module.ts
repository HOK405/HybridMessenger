import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { ReactiveFormsModule } from '@angular/forms';
import { HttpClientModule } from '@angular/common/http';

import { AppComponent } from './app.component';
import { LoginComponent } from './pages/login/login.component';
import { AppRoutingModule } from './app-routing.module';
import { UserSearchComponent } from './pages/user-search/user-search.component';
import { RegisterComponent } from './pages/register/register.component';
import { UserMessagesComponent } from './pages/user-messages/user-messages.component';
import { NgMultiSelectDropDownModule } from 'ng-multiselect-dropdown';
import { SidebarComponent } from './shared/components/sidebar/sidebar.component';
import { GrowDirective } from './shared/directives/grow.directive';
import { HasPermissionDirective } from './shared/directives/has-permission.directive';
import { CountryNumberFormatPipe } from './shared/pipes/country-number-format.directive';
import { InternationalDatePipe } from './shared/pipes/international-date.directive';

@NgModule({
  declarations: [
    AppComponent,
    LoginComponent,
    UserSearchComponent,
    RegisterComponent,
    UserMessagesComponent,
    SidebarComponent,
    GrowDirective,
    HasPermissionDirective,
    CountryNumberFormatPipe,
    InternationalDatePipe,
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