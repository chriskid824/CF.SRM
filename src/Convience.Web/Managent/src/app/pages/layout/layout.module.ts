import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { LayoutComponent } from './layout/layout.component';
import { AppCommonModule } from '../app-common/app-common.module';
import { NzAvatarModule } from 'ng-zorro-antd/avatar';
import { NzBreadCrumbModule } from 'ng-zorro-antd/breadcrumb';
import { NzButtonModule } from 'ng-zorro-antd/button';
import { NzCardModule } from 'ng-zorro-antd/card';
import { NzDropDownModule } from 'ng-zorro-antd/dropdown';
import { NzFormModule } from 'ng-zorro-antd/form';
import { NzIconModule } from 'ng-zorro-antd/icon';
import { NzInputModule } from 'ng-zorro-antd/input';
import { NzMessageModule } from 'ng-zorro-antd/message';
import { NzModalModule } from 'ng-zorro-antd/modal';
import { NzLayoutModule } from 'ng-zorro-antd/layout';
import { NzBadgeModule } from 'ng-zorro-antd/badge';
import { NzTagModule } from 'ng-zorro-antd/tag';
import { RouterModule } from '@angular/router';
//import { SharedModule } from '@ngx-translate/core';
import { TranslateModule } from '@ngx-translate/core';
import { NavbarComponent } from './navbar/navbar.component';
import {MatToolbarModule} from '@angular/material/toolbar';
import { MatMenuModule } from '@angular/material/menu';
@NgModule({
  declarations: [
    LayoutComponent,
    NavbarComponent
  ],
  imports: [
    CommonModule,
    RouterModule,
    AppCommonModule,
    NzFormModule,
    NzBreadCrumbModule,
    NzButtonModule,
    NzLayoutModule,
    NzDropDownModule,
    NzCardModule,
    NzAvatarModule,
    NzBadgeModule,
    NzIconModule,
    NzMessageModule,
    NzButtonModule,
    NzInputModule,
    NzModalModule,
    NzTagModule,
    TranslateModule,
    MatToolbarModule,
    MatMenuModule,
  ],
  exports: [
    LayoutComponent
  ]
})
export class LayoutModule { }
