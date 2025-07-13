//
//  OnaWidget.swift
//  widget
//
//  Created by Natalia Naumova on 02/08/2024.
//

import WidgetKit
import SwiftUI

struct Provider: TimelineProvider {
    func placeholder(in context: Context) -> PeriodStateEntry {
        PeriodStateEntry(date: Date(), cycleDay: 7, daysToNext: 21)
    }

    func getSnapshot(in context: Context, completion: @escaping (PeriodStateEntry) -> ()) {
        let entry = PeriodStateEntry(date: Date(), cycleDay: 7, daysToNext: 21)
        completion(entry)
    }

    func getTimeline(in context: Context, completion: @escaping (Timeline<Entry>) -> ()) {
        let periodState = getPeriodState();
        let entry = periodState == nil ? PeriodStateEntry(date: Date(), cycleDay: 7, daysToNext: 28) : getEntry(periodState: periodState!)
        
        let calendar = Calendar.current
        let tomorrow = calendar.date(byAdding: .day, value: 1, to: calendar.startOfDay(for: Date()))

        let timeline = Timeline(entries: [entry], policy: .after(tomorrow!))
        
        completion(timeline)
    }
    
    func getEntry(periodState: PeriodState) -> PeriodStateEntry {
        let dateFormatter = DateFormatter()
        dateFormatter.dateFormat = "yyyy'-'MM'-'dd"
        let currentPeriodStart = dateFormatter.date(from: periodState.startDate)!
        
        let calendar = Calendar.current
        let nextPeriodStart = calendar.date(byAdding: .day, value: periodState.interval, to: currentPeriodStart)!
        let cycleDay = calendar.dateComponents([.day], from: currentPeriodStart, to: Date()).day! + 1
        let daysToNext = calendar.dateComponents([.day], from: Date(), to: nextPeriodStart).day! + 1

        return PeriodStateEntry(date: Date(), cycleDay: cycleDay, daysToNext: daysToNext)
    }
    
    func getPeriodState() -> PeriodState? {
        guard let userDefault = UserDefaults(suiteName: "group.com.natalianaumova.ona")?.string(forKey: "PeriodState") else {
            return nil
        }
        let data = Data(userDefault.utf8)
        let periodState = try? JSONDecoder().decode(PeriodState.self, from: data)
        return periodState ?? nil
    }
}

struct PeriodStateEntry: TimelineEntry {
    let date: Date
    let cycleDay: Int
    let daysToNext: Int
}

struct OnaWidgetEntryView : View {
    var entry: Provider.Entry

   
    var body: some View {
        ZStack(alignment: .topLeading) {
            Circle()
                .fill(Color(red: 241/255, green: 247/255, blue: 1/255))
                .frame(width: 36, height: 36)
                .offset(x: 0, y: 0)
            
            VStack {
                Text("Cycle day \(entry.cycleDay)")
                    .padding(.bottom, 2)
                Text("\(entry.daysToNext) days")
                    .font(.title)
                Text("until next period")
                    .font(.footnote)
            }
            .frame(maxWidth: .infinity, maxHeight: .infinity)
        }
        .frame(maxWidth: .infinity, maxHeight: .infinity, alignment: .topLeading)
    }
}

struct OnaWidget: Widget {
    let kind: String = "PeriodTimes"

    var body: some WidgetConfiguration {
        StaticConfiguration(kind: kind, provider: Provider()) { entry in
            if #available(iOS 17.0, *) {
                OnaWidgetEntryView(entry: entry)
                    .containerBackground(.fill.tertiary, for: .widget)
            } else {
                OnaWidgetEntryView(entry: entry)
                    .padding()
                    .background()
            }
        }
        .configurationDisplayName("My Widget")
        .description("This is an example widget.")
    }
}

struct PeriodState : Codable {
    var startDate: String
    var duration: Int
    var interval: Int
}

// #Preview(as: .systemSmall) {
//     OnaWidget()
// } timeline: {
//     PeriodStateEntry(date: .now, cycleDay: 7, daysToNext: 21)
// }
