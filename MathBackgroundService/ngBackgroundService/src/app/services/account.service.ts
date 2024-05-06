import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { lastValueFrom } from 'rxjs';
import { environment } from 'src/environments/environment';

const CONNECTION_LOCALSTORAGE_KEY = "connected";

@Injectable({
  providedIn: 'root'
})
export class AccountService {

  apiBaseUrl = environment.apiUrl + "api/"
  accountBaseUrl = this.apiBaseUrl + "Account/";

  username = "test";
  motDePasse = "Passw0rd!";

  constructor(public http: HttpClient) { }

  async register(){
    let registerData = {
      username: this.username,
      email : this.username + "@test.com",
      password : this.motDePasse,
      passwordConfirm : this.motDePasse,
    }
    await lastValueFrom(this.http.post<any>(this.accountBaseUrl + 'Register', registerData));
  }

  async login(){
    let registerData = {
      username : this.username,
      password : this.motDePasse
    }
    await lastValueFrom(this.http.post<any>(this.accountBaseUrl + 'Login', registerData));
    localStorage.setItem(CONNECTION_LOCALSTORAGE_KEY, registerData.username);
  }

  async logout(){
    await lastValueFrom(this.http.get<any>(this.accountBaseUrl + 'Logout'));
    localStorage.removeItem(CONNECTION_LOCALSTORAGE_KEY);
  }

  isLoggedIn() : Boolean{
    return localStorage.getItem(CONNECTION_LOCALSTORAGE_KEY) != null;
  }

}
