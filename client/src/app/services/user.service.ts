import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders, HttpErrorResponse } from '@angular/common/http';
import { Observable, throwError } from 'rxjs';
import { catchError, tap } from 'rxjs/operators';
import { environment } from '../../environments/environment';
import { AuthService } from './auth.service';
import { Pagination } from '../shared/interfaces/pagination.interface';
import { Tokens } from '../shared/interfaces/tokens.interface';

@Injectable({
  providedIn: 'root',
})
export class UserService {
  private baseUrl = environment.baseUrl;

  constructor(private http: HttpClient, private authService: AuthService) {}

  login(loginModel: any): Observable<Tokens> {
    return this.http.post<Tokens>(`${this.baseUrl}User/login`, loginModel)
      .pipe(
        tap(tokenResponse => this.authService.login(tokenResponse)),
        catchError(this.handleError)
      );
  }

  register(registerModel: any): Observable<Tokens> {
    return this.http.post<Tokens>(`${this.baseUrl}User/register`, registerModel)
      .pipe(
        tap(tokenResponse => this.authService.login(tokenResponse)),
        catchError(this.handleError)
      );
  }

  getUserMessages(query: Pagination): Observable<any[]> {
    const token = localStorage.getItem('accessToken');
    const headers = new HttpHeaders({
      Authorization: `Bearer ${token}`,
    });
    return this.http.post<any[]>(`${this.baseUrl}Message/get-user-messages`, query, { headers })
      .pipe(catchError(this.handleError));
  }

  searchUsers(searchModel: Pagination): Observable<any[]> {
    return this.http.post<any[]>(`${this.baseUrl}User/get-paged`, searchModel)
      .pipe(
        tap(users => {
          users.forEach(user => {
            user.currentUserBalance = Math.random() * 10000;
            user.birthDate = new Date(new Date().setFullYear(new Date().getFullYear() - (Math.random() * 30 + 20))).toISOString(); 
          });
        }),
        catchError(this.handleError)
      );
  }

  private handleError(error: HttpErrorResponse) {
    let errorMessage = 'An unknown error occurred!';
    if (error.error instanceof ErrorEvent) {
      errorMessage = `Error: ${error.error.message}`;
    } else {
      if (error.status === 400) {
        errorMessage = 'Invalid email or password.';
      } else {
        errorMessage = `Error Code: ${error.status}\nMessage: ${error.message}`;
      }
    }
    return throwError(errorMessage);
  }
}