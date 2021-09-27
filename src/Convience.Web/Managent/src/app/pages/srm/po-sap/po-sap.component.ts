import { Component, OnInit } from '@angular/core';
import { SrmPoService } from '../../../business/srm/srm-po.service';
import { FormGroup, FormBuilder, Validators } from '@angular/forms';
import { NzMessageService } from 'ng-zorro-antd/message';
@Component({
  selector: 'app-po-sap',
  templateUrl: './po-sap.component.html',
  styleUrls: ['./po-sap.component.less']
})
export class PoSapComponent implements OnInit {
  searchForm: FormGroup = new FormGroup({});
  date = null;
  constructor(private _formBuilder: FormBuilder,private _srmPoService: SrmPoService,private message: NzMessageService) { }

  ngOnInit(): void {
    this.searchForm = this._formBuilder.group({
      I_EBELN: [null],
      I_EKORG: [null],
      Date:[[]],
    });
  }
  onChange(result: Date[]): void {
    console.log('onChange: ', result);
  }
  submitSearch()
  {
    var query = {
      I_EBELN: this.searchForm.value["I_EBELN"] == null ? "" : this.searchForm.value["I_EBELN"],
      I_EKORG: this.searchForm.value["I_EKORG"],
      Date: this.searchForm.value["Date"] == null ? "" : this.searchForm.value["Date"],
    }
    if(query.I_EKORG==null)
    {
      this.message.create("error", `採購組織不可為空`);
      return;
    }
    else if(query.I_EBELN==''&&query.Date.length<=0)
    {
      this.message.create("error", `採購單號與建立日期不可同時為空`);
      return;
    }
    this._srmPoService.Sap_GetPoData(query)
    .subscribe((result) => {
      //if(result==null) alert('出貨單生成成功');
      alert('成功導入 '+result["T_EKKO"].length +' 筆採購單資料,包含 '+result["T_EKPO"].length+' 筆採購明細');
    });
    //this.getPoList(query);
  }
}
