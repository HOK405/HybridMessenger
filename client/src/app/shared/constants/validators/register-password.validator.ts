import { AbstractControl, ValidatorFn } from '@angular/forms';

export function passwordValidator(): ValidatorFn {
  return (control: AbstractControl): { [key: string]: any } | null => {
    const value = control.value;
    if (!value) {
      return null;
    }

    const hasDigit = /\d/.test(value);
    const hasLowerCase = /[a-z]/.test(value);
    const isValidLength = value.length >= 6;

    const valid = hasDigit && hasLowerCase && isValidLength;

    if (!valid) {
      return { passwordInvalid: true };
    }
    return null;
  };
}