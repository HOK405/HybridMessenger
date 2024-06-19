import { Injectable } from '@angular/core';
import { BehaviorSubject, Observable } from 'rxjs';
import { TokenResponse} from '../Models/Responses/TokenResponse';

@Injectable({
  providedIn: 'root',
})
export class AuthService {
  private authStatusSubject = new BehaviorSubject<boolean>(this.hasToken());
  authStatus$: Observable<boolean> = this.authStatusSubject.asObservable();

  constructor() {}

  private hasToken(): boolean {
    return !!localStorage.getItem('accessToken');
  }

  setTokens(accessToken: string, refreshToken: string): void {
    localStorage.setItem('accessToken', accessToken);
    localStorage.setItem('refreshToken', refreshToken);
  }

  login(tokenResponse: TokenResponse): void {
    this.setTokens(tokenResponse.accessToken, tokenResponse.refreshToken);
    this.authStatusSubject.next(true);
  }

  logout(): void {
    localStorage.removeItem('accessToken');
    localStorage.removeItem('refreshToken');
    this.authStatusSubject.next(false);
  }
}
