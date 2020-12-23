import { Component, OnInit } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Router } from '@angular/router';
import { User } from 'src/app/models/user.model';
import { Observable } from 'rxjs';

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.css']
})
export class HomeComponent implements OnInit {

  public role: string | null;
  public userId: number | null;
  public user!: Observable<User>;

  constructor(
    private _http: HttpClient,
    private _router: Router
  ) {
    this.role = localStorage.getItem('role');
    this.userId = Number(localStorage.getItem('id'));    
  }

  ngOnInit(): void {
    this.getUser();
  }

  signout() {
    localStorage.removeItem('role');
    localStorage.removeItem('id');
    window.location.reload();
  }

  private getUser() {
    if (!this.userId) {
      return;
    }
    this.user = this._http.get(`https://localhost:44309/api/payments/user/${this.userId}`) as Observable<User>;
  }
}
