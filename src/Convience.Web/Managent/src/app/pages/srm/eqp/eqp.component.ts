import { Component, OnInit, TemplateRef } from '@angular/core';
import { GridOptions } from 'ag-grid-community';
import { Role } from 'src/app/pages/system-manage/model/role';
import { FormGroup, FormBuilder, Validators } from '@angular/forms';
import { NzModalRef, NzModalService } from 'ng-zorro-antd/modal';
import { RoleService } from 'src/app/business/system-manage/role.service';
import { MenuService } from 'src/app/business/system-manage/menu.service';
import { NzMessageService } from 'ng-zorro-antd/message';
import { Material } from 'src/app/pages/srm/model/material';
import { SrmQotService } from '../../../business/srm/srm-qot.service';
import { Process } from '../model/Process';
import { Surface } from '../model/Surface';
import { Other } from '../model/Other';
//import { NzTreeNodeOptions } from 'ng-zorro-antd/tree';
import { Qot, QotH, QotV, reject } from '../model/Qot';
import { ViewChild } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { NzTreeNodeOptions } from 'ng-zorro-antd/tree';
import { runInThisContext } from 'node:vm';
import { StorageService } from '../../../services/storage.service';
import { LayoutComponent } from '../../layout/layout/layout.component';
import { SrmModule } from '../srm.module';


@Component({
  selector: 'app-eqp',
  templateUrl: './eqp.component.html',
  styleUrls: ['./eqp.component.less']
})
export class EqpComponent implements OnInit {
  Quality_Problems: FormGroup = new FormGroup({});
  constructor() { 
   
  }

  ngOnInit(): void {
  }

}
