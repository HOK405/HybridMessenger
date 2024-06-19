import { Component } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { UserService } from '../../Services/user.service';
import { passwordValidator } from '../../Validators/register-password.validator';

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
    private router: Router
  ) {
    this.registerForm = this.fb.group({
      userName: ['', [Validators.required]],
      email: ['', [Validators.required, Validators.email]],
      password: ['', [Validators.required, passwordValidator()]],
      phoneNumber: [''],
    });
  }

  handleRegister(): void {
    if (this.registerForm.invalid) {
      return;
    }

    const registerModel = this.registerForm.value;
    this.userService.register(registerModel).subscribe({
      next: () => {
        this.router.navigate(['/logged-in']);
      },
      error: (error) => {
        this.registerResult = error;
      }
    });
  }
}