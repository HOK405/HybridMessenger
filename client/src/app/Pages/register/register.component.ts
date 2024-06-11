import { Component } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { HttpService } from '../../services/http.service';
import { AuthService } from '../../services/auth.service';

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
    private httpService: HttpService,
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
    this.httpService.post<any>('User/register', registerModel).subscribe(
      (response) => {
        this.registerResult = 'Registered successfully!';
        this.router.navigate(['/logged-in']);
      },
      (error) => {
        this.registerResult = `Registration failed: ${error.message}`;
      }
    );
  }
}
