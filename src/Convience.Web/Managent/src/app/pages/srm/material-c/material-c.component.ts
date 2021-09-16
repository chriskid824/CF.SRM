import { NgIf } from '@angular/common';
import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, FormArray, FormControl} from '@angular/forms';
import { NzMessageService } from 'ng-zorro-antd/message';
import { SrmMaterialService } from '../../../business/srm/srm-material.service';

@Component({
  selector: 'app-material-c',
  templateUrl: './material-c.component.html',
  styleUrls: ['./material-c.component.less']
})
export class MaterialCComponent implements OnInit {
  formDetail: FormGroup = new FormGroup({});
  searchForm: FormGroup = new FormGroup({});

  constructor(
    private _formBuilder: FormBuilder,
    private _messageService: NzMessageService,
    private _srmMaterialService: SrmMaterialService,) { 
           
    }

  ngOnInit(): void {
    this.searchForm = this._formBuilder.group({
      material: [null],
      NgIf:true
    });
  }


  add() {
    $('#addBar').hide();
    var query = {
      material: this.searchForm.value["material"] == null ? "" : this.searchForm.value["material"],
    }
    if (!this.searchForm.value["material"]) {
      alert("請輸入料號");
      return;
    }
    //console.log(query);
    this._srmMaterialService.CheckMatnr(query).subscribe(result => {
      console.log('123456789')
      $('#addBar').show();
      //this._srmMaterialService.AddMatnr(query).subscribe(result => {
      //  console.log('123456789')
        
      //});

    });
  }

}
