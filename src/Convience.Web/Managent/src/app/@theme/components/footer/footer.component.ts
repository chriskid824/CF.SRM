import { Component } from '@angular/core';

@Component({
  selector: 'ngx-footer',
  styleUrls: ['./footer.component.scss'],
  template: `
  <span class="created-by">
  Created with <b><a href="https://www.chenfull.com.tw/" target="_blank">ChenFull</a></b> 2021
</span>
  `,
})
export class FooterComponent {
}
