import { Component, OnInit } from '@angular/core';
import { FormGroup, FormControl, Validators } from '@angular/forms';
import { HttpClient } from '@angular/common/http';
import { Router } from '@angular/router';
import { SignUpModel } from 'src/app/models/sign-up.model';
import { SessionData } from 'src/app/models/session-data.model';

@Component({
  selector: 'app-sign-up',
  templateUrl: './sign-up.component.html',
  styleUrls: ['./sign-up.component.css']
})
export class SignUpComponent implements OnInit {

  formGroup!: FormGroup;
  confirmPassword!: string;
  miss: boolean = false;

  constructor(
    private _http: HttpClient,
    private _router: Router
  ) { }

  ngOnInit(): void {
    this.formInitBuild();
  }

  signup() {
    if (this.formGroup.controls['password'].value != this.confirmPassword){
      this.miss = true;
      return;
    }

    this.miss = false;
    let newUser = Object.assign(new SignUpModel(), this.formGroup.value)

    this._http.post('https://localhost:44309/api/payments/signup', newUser)
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
      'fullName': new FormControl('', [Validators.required]),
      'password': new FormControl('', [Validators.required]),
    });
  }
}
