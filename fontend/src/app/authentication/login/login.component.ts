import { Component, OnInit, ViewEncapsulation } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { Router, ActivatedRoute } from '@angular/router';
import { first } from 'rxjs/operators';


import { AuthenticationService } from '@app/_services/authentication.service';

@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.scss'],
  encapsulation: ViewEncapsulation.None
})
export class LoginComponent implements OnInit {
  loading = false;
  submitted = false;
  error = '';
  loginForm!: FormGroup;
  returnUrl!: string;

  constructor(
    private formBuilder: FormBuilder,
    private route: ActivatedRoute,
    private router: Router,
    private authenticationService: AuthenticationService
    ) { 
      // redirect to home if already logged in
      // TODO: Update cho cac role khac
      if (this.authenticationService.currentUserValue) { 
        if (this.authenticationService.currentUserValue.mainRole === "Admin") {
          this.router.navigate(['/account-list']);
        } else {
          this.router.navigate(['/home-user']);
        }
      }
    }

  ngOnInit(): void {
    this.loginForm = this.formBuilder.group({
      emailAddress: ['', [Validators.required, Validators.email]],
      password: ['', Validators.required]
    });

    // get return url from route parameters or default to '/'
    this.returnUrl = this.route.snapshot.queryParams['returnUrl'];
  }

  // convenience getter for easy access to form fields
  get f() { return this.loginForm.controls; }

  onSubmit() {
    this.submitted = true;

    // stop here if form is invalid
    if (this.loginForm.invalid) {
        return;
    }

    this.loading = true;
    this.authenticationService.login(this.f.emailAddress.value, this.f.password.value)
        .pipe(first())
        .subscribe(
            (data: any) => {
              if (this.returnUrl) {
                this.router.navigate([this.returnUrl]);
              } else {
                if (data.mainRole === "Admin") {
                  this.router.navigate(['/account-list']);
                } else {
                  this.router.navigate(['/home-user']);
                }
              }
            },
            (error: any) => {
              this.error = error;
              this.loading = false;
            });
    }

}
