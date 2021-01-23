import { Component, OnInit, Input } from "@angular/core";
import { ChartConfig } from "../globals/chart-classes";
import { ChartDataService } from "../services/chart-data.service";

@Component({
  selector: "app-chroma-charts",
  templateUrl: "./chroma-charts.component.html",
  styleUrls: ["./chroma-charts.component.css"]
})
export class ChromaChartsComponent implements OnInit {
  @Input() Config: ChartConfig;

  data: any[] = [];
  colorScheme: any;

  onSelect() {}

  onActivate() {}

  onDeactivate() {}

  constructor(private service: ChartDataService) {}

  ngOnInit() {
    this.service.getData(this.Config).subscribe(data => {
      this.data = data;
    });
    this.service.getColorScheme(this.Config).subscribe(scheme => {
      this.colorScheme = scheme;
    });
  }
}
