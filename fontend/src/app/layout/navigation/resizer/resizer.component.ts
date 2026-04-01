import { Component, OnInit, Input } from '@angular/core';

@Component({
  selector: 'app-resizer',
  templateUrl: './resizer.component.html',
  styleUrls: ['./resizer.component.css']
})
export class ResizerComponent implements OnInit {
  @Input() expanded!: boolean;

  constructor() { }

  ngOnInit(): void {
  }

}
