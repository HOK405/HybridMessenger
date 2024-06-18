import { Component } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { AuthService } from '../../services/auth.service';
import { UserService } from '../../services/user.service';

@Component({
  selector: 'app-register',
  templateUrl: './register.component.html',
  styleUrls: ['./register.component.css'],
})
export class RegisterComponent {
  registerForm: FormGroup;
  registerResult: string | null = null;

  constructor(
    private fb: FormBuilder,
    private userService: UserService,
    private authService: AuthService,
    private router: Router
  ) {
    this.registerForm = this.fb.group({
      userName: ['', [Validators.required]],
      email: ['', [Validators.required, Validators.email]],
      password: ['', [Validators.required]],
      phoneNumber: [''],
    });
  }

  get alertClass(): string {
    return this.registerResult === 'Registered successfully!'
      ? 'alert-success'
      : 'alert-danger';
  }

  handleRegister(): void {
    if (this.registerForm.invalid) {
      return;
    }

    const registerModel = this.registerForm.value;
    this.userService.register(registerModel).subscribe(
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
          this.registerResult = 'Registered successfully!';
          this.router.navigate(['/logged-in']);
        } else {
          this.registerResult =
            'Registration failed: Missing tokens in response.';
        }
      },
      (error) => {
        this.registerResult = `Registration failed: ${error.message}`;
      }
    );
  }
}
