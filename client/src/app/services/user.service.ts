import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../environments/environment';

interface TokenResponse {
  accessToken: string;
  refreshToken: string;
}

interface PaginationRequest {
  pageNumber: number;
  pageSize: number;
  sortBy: string;
  searchValue: string;
  ascending: boolean;
  fields: string[];
}

@Injectable({
  providedIn: 'root',
})
export class UserService {
  private baseUrl = environment.baseUrl;

  constructor(private http: HttpClient) {}

  login(loginModel: any): Observable<TokenResponse> {
    return this.http.post<TokenResponse>(
      `${this.baseUrl}User/login`,
      loginModel
    );
  }

  register(registerModel: any): Observable<TokenResponse> {
    return this.http.post<TokenResponse>(
      `${this.baseUrl}User/register`,
      registerModel
    );
  }

  getUserMessages(query: PaginationRequest): Observable<any[]> {
    const token = localStorage.getItem('accessToken');
    const headers = new HttpHeaders({
      Authorization: `Bearer ${token}`,
    });
    return this.http.post<any[]>(
      `${this.baseUrl}Message/get-user-messages`,
      query,
      { headers }
    );
  }

  searchUsers(searchModel: PaginationRequest): Observable<any[]> {
    return this.http.post<any[]>(`${this.baseUrl}User/get-paged`, searchModel);
  }
}
