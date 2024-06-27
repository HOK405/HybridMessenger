import { Pipe, PipeTransform } from '@angular/core';

@Pipe({
  name: 'countryNumberFormat'
})
export class CountryNumberFormatPipe implements PipeTransform {
  transform(value: number, locale: string): string {
    return new Intl.NumberFormat(locale).format(value);
  }
}