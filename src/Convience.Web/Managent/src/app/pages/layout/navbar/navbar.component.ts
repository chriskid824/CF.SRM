import { Component, OnInit,Input } from '@angular/core';
import { Observable } from 'rxjs/internal/Observable';
import { LanguageService } from 'src/app/business/language.service';
import { Menu } from '../../system-manage/model/menu';
@Component({
  selector: 'app-navbar',
  templateUrl: './navbar.component.html',
  styleUrls: ['./navbar.component.less']
})
export class NavbarComponent implements OnInit {
  @Input() value: Observable<Menu[]>;
  @Input() menu;
  @Input() isMobile;
  get currentLanguage() {
    console.info(this.languageService.translate.currentLang);
    return this.languageService.translate.currentLang;
  }

  constructor( private languageService: LanguageService ) {
  }

  getCountryMap(currentLanguage: string) {
    console.info(this.languageService.countryMap.get(currentLanguage));
    return this.languageService.countryMap.get(currentLanguage);
  }
  useLanguage(language: string) {
    this.languageService.setLang(language);
  }

  ngOnInit() {

  }
}
