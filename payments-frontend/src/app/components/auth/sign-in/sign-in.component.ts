import { Component, OnInit } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { FormGroup, FormControl, Validators } from '@angular/forms';
import { Credentials } from 'src/app/models/credentials.model';
import { SessionData } from 'src/app/models/session-data.model';
import { Router } from '@angular/router';

@Component({
  selector: 'app-sign-in',
  templateUrl: './sign-in.component.html',
  styleUrls: ['./sign-in.component.css']
})
export class SignInComponent implements OnInit {

  formGroup!: FormGroup;

  constructor(
    private _http: HttpClient,
    private _router: Router
  ) { }

  ngOnInit(): void {
    this.formInitBuild();
  }

  signin() {
    let cred = Object.assign(new Credentials(), this.formGroup.value);
    this._http.post('https://localhost:44309/api/payments/signin', cred)
      .subscribe(data => {
        let sessionData = data as SessionData;
        if (sessionData && sessionData.userId && sessionData.userRole) {
          localStorage.setItem('id', sessionData.userId.toString());
          localStorage.setItem('role', sessionData.userRole);
          this._router.navigate(['/']);
        }
      });
  }

  private formInitBuild(data: any = {}): void {
    this.formGroup = new FormGroup({
      'login': new FormControl('', [Validators.required]),
      'password': new FormControl('', [Validators.required]),
    });
  }
}
