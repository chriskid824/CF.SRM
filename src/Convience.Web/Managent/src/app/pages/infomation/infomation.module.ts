import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { DashboardComponent } from './dashboard/dashboard.component';
import { RouterModule } from '@angular/router';
import { LoginGuard } from 'src/app/guards/login.guard';
import { AppCommonModule } from '../app-common/app-common.module';
import { NzCardModule } from 'ng-zorro-antd/card';
import { NzGridModule } from 'ng-zorro-antd/grid';
import { NzStatisticModule } from 'ng-zorro-antd/statistic';
import { AnnouncementComponent } from './announcement/announcement.component';
import { MatTabsModule } from '@angular/material/tabs';
import {ScrollingModule} from '@angular/cdk/scrolling';

@NgModule({
  declarations: [
    DashboardComponent,
    AnnouncementComponent
  ],
  imports: [
    CommonModule,
    AppCommonModule,
    MatTabsModule,
    RouterModule.forChild([
      { path: '', pathMatch: 'full', redirectTo: 'dashboard' },
      { path: 'dashboard', component: DashboardComponent, canActivate: [LoginGuard] }
    ]),

    // NGZorro组件
    NzStatisticModule,
    NzCardModule,
    NzGridModule,
    ScrollingModule
  ]
})
export class InfomationModule { }
