import { Component } from '@angular/core';
import { LanguageService } from './business/language.service';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.less']
})
export class AppComponent {
  constructor(private languageService: LanguageService) {
    this.languageService.setInitState();
  }
}
