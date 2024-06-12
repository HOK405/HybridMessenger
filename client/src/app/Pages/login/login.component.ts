import { Component } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { HttpClient } from '@angular/common/http';
import { AuthService } from '../../services/auth.service';
import { environment } from '../../../environments/environment';

interface TokenResponse {
  accessToken: string;
  refreshToken: string;
}

@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.css'],
})
export class LoginComponent {
  loginForm: FormGroup;
  loginResult: string | null = null;
  private baseUrl = environment.baseUrl;

  constructor(
    private fb: FormBuilder,
    private http: HttpClient,
    private authService: AuthService,
    private router: Router
  ) {
    this.loginForm = this.fb.group({
      email: ['', [Validators.required, Validators.email]],
      password: ['', [Validators.required]],
    });
  }

  get alertClass(): string {
    return this.loginResult === 'Logged in successfully!'
      ? 'alert-success'
      : 'alert-danger';
  }

  handleLogin(): void {
    if (this.loginForm.invalid) {
      return;
    }

    const loginModel = this.loginForm.value;
    this.http
      .post<TokenResponse>(`${this.baseUrl}User/login`, loginModel)
      .subscribe(
        (tokenResponse) => {
          if (
            tokenResponse &&
            tokenResponse.accessToken &&
            tokenResponse.refreshToken
          ) {
            this.authService.login(tokenResponse.accessToken);
            this.authService.setTokens(
              tokenResponse.accessToken,
              tokenResponse.refreshToken
            );
            this.loginResult = 'Logged in successfully!';
            this.router.navigate(['/logged-in']);
          } else {
            this.loginResult =
              "You aren't logged in. Incorrect email or password.";
          }
        },
        (error) => {
          if (error.status === 400 && error.error.errors) {
            const validationErrors = error.error.errors;
            this.loginResult = Object.keys(validationErrors)
              .map((key) => validationErrors[key].join(' '))
              .join(' ');
          } else {
            this.loginResult = `Login failed: ${error.message}`;
          }
        }
      );
  }
}
