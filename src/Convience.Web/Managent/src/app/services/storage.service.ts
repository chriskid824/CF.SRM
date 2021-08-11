import { Injectable } from '@angular/core';
import { JwtHelperService } from "@auth0/angular-jwt";

@Injectable({
  providedIn: 'root'
})
export class StorageService {

  jwtService = new JwtHelperService();

  constructor() { }

  set userToken(value) {
    let expirationDate = this.jwtService.getTokenExpirationDate(value);
    let decodeToken = this.jwtService.decodeToken(value);

    localStorage.setItem('userToken', value);
    localStorage.setItem("userroles", decodeToken['userroleids']);
    localStorage.setItem("username", decodeToken['username']);
    localStorage.setItem('userTokenExpiration', expirationDate.toString());
    localStorage.setItem("costNo", decodeToken['costNo']);
    localStorage.setItem("werks", decodeToken['werks']);
  }

  removeUserToken() {
    localStorage.removeItem('userToken');
    localStorage.removeItem('userroles');
    localStorage.removeItem('username');
    localStorage.removeItem('userTokenExpiration');
    localStorage.removeItem('costNo');
    localStorage.removeItem('werks');

    localStorage.removeItem("name");
    localStorage.removeItem("avatar");
    localStorage.removeItem("identifycation");
    localStorage.removeItem("route");
  }

  get userToken() {
    return localStorage.getItem('userToken');
  }

  get tokenExpiration() {
    return localStorage.getItem('userTokenExpiration');
  }

  get userName() {
    return localStorage.getItem("username");
  }

  get IsTokenExpire() {
    let now = new Date();
    let expire = new Date(this.tokenExpiration);
    return now > expire;
  }

  get costNo() {
    return localStorage.getItem("costNo");
  }
  set costNo(value) {
    localStorage.setItem("costNo", value);
  }

  get werks() {
    return localStorage.getItem("werks");
  }
  set werks(value) {
    localStorage.setItem("werks", value);
  }

  hasUserToken(): boolean {
    return this.userToken != null;
  }

  get Name() {
    return localStorage.getItem("name");
  }
  set Name(value) {
    localStorage.setItem("name", value);
  }

  get UserRoles() {
    return localStorage.getItem("userroles");
  }

  get Avatar() {
    return localStorage.getItem("avatar");
  }
  set Avatar(value) {
    localStorage.setItem("avatar", value);
  }

  get Identifycation() {
    return localStorage.getItem("identifycation");
  }
  set Identifycation(value) {
    localStorage.setItem("identifycation", value);
  }

  get Route() {
    return localStorage.getItem("route");
  }
  set Route(value) {
    localStorage.setItem("route", value);
  }

  clearTinymceCache() {
    for (let i = 0; i < localStorage.length; i++) {
      let key = localStorage.key(i);
      if (key.startsWith('tinymce-autosave') || key.startsWith('tinymce-url')) {
        localStorage.removeItem(key);
      }
    }
  }
}
