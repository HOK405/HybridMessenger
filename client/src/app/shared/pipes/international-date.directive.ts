import { Pipe, PipeTransform } from '@angular/core';

@Pipe({
  name: 'internationalDate'
})
export class InternationalDatePipe implements PipeTransform {
  transform(value: string, locale: string): string {
    const date = new Date(value);
    return new Intl.DateTimeFormat(locale, { year: 'numeric', month: 'long', day: 'numeric' }).format(date);
  }
}