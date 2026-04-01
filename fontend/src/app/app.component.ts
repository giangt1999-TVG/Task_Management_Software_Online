import { Component, OnInit } from '@angular/core';
import { PrimeNGConfig } from 'primeng/api';
import { MessagingService } from './_services/messaging.service';



@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css'],
})

export class AppComponent implements OnInit {
  title = 'TMSO Sofware';


  constructor(
    private primengConfig: PrimeNGConfig,
    private messagingService: MessagingService
  ) {
  }

  ngOnInit() {
    this.primengConfig.ripple = true;

    this.messagingService.requestPermission();
    this.messagingService.receiveMessage();
  }

}



