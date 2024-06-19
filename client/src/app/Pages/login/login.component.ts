import { Component } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { UserService } from '../../Services/user.service';

@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.css'],
})
export class LoginComponent {
  loginForm: FormGroup;
  loginResult: string | null = null;

  constructor(
    private fb: FormBuilder,
    private userService: UserService,
    private router: Router
  ) {
    this.loginForm = this.fb.group({
      email: ['', [Validators.required, Validators.email]],
      password: ['', [Validators.required]],
    });
  }

  handleLogin(): void {
    if (this.loginForm.invalid) {
      return;
    }

    const loginModel = this.loginForm.value;
    this.userService.login(loginModel).subscribe({
      next: () => {
        this.router.navigate(['/logged-in']);
      },
      error: (error) => {
        this.loginResult = error;
      }
    });
  }
}