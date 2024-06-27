import { Injectable } from '@angular/core';
import { BehaviorSubject, Observable } from 'rxjs';
import { Tokens } from '../shared/interfaces/tokens.interface';
import { jwtDecode } from 'jwt-decode';

@Injectable({
  providedIn: 'root',
})
export class AuthService {
  private authStatusSubject = new BehaviorSubject<boolean>(this.hasToken());
  authStatus$: Observable<boolean> = this.authStatusSubject.asObservable();
  private userPermissionsSubject = new BehaviorSubject<string[]>(this.getUserPermissions());
  userPermissions$: Observable<string[]> = this.userPermissionsSubject.asObservable();

  constructor() {}

  private hasToken(): boolean {
    return !!localStorage.getItem('accessToken');
  }

  private getUserPermissions(): string[] {
    const token = localStorage.getItem('accessToken');
    if (token) {
      const decoded: any = jwtDecode(token);
      const email = decoded.email;
      if (email.endsWith('@admin.com')) {
        return ['Read', 'Write', 'Delete'];
      } else {
        return ['Read'];
      }
    }
    return [];
  }

  setTokens(accessToken: string, refreshToken: string): void {
    localStorage.setItem('accessToken', accessToken);
    localStorage.setItem('refreshToken', refreshToken);
    this.authStatusSubject.next(true);
    this.userPermissionsSubject.next(this.getUserPermissions());
  }

  login(tokenResponse: Tokens): void {
    this.setTokens(tokenResponse.accessToken, tokenResponse.refreshToken);
  }

  logout(): void {
    localStorage.removeItem('accessToken');
    localStorage.removeItem('refreshToken');
    this.authStatusSubject.next(false);
    this.userPermissionsSubject.next([]);
  }
}
