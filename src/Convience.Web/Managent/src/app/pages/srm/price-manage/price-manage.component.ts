import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup } from '@angular/forms';
import { Router } from '@angular/router';
import { LayoutComponent } from '../../layout/layout/layout.component';
import { NzModalRef, NzModalService } from 'ng-zorro-antd/modal';
import { SrmRfqService } from '../../../business/srm/srm-rfq.service';
import { StorageService } from '../../../services/storage.service';

@Component({
  selector: 'app-price-manage',
  templateUrl: './price-manage.component.html',
  styleUrls: ['./price-manage.component.less']
})
export class PriceManageComponent implements OnInit {

  constructor(private _formBuilder: FormBuilder
    , private _router: Router
    , private _layout: LayoutComponent
    , private _storageService: StorageService
  ) { }

  ngOnInit(): void {
  }
}
