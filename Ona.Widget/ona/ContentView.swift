//
//  ContentView.swift
//  ona
//
//  Created by Natalia Naumova on 29/07/2024.
//

import SwiftUI

struct ContentView: View {
    @State private var count = UserDefaults(suiteName: "group.com.natalianaumova.ona")!.integer(forKey: "Count");
    //@Binding private var periodState: PeriodState
    //@Binding private var start: String
    //@Binding private var duration: String
    //@Binding private var interval: String
    
    var body: some View {
        //periodState = getPeriodState()
        //start = periodState.start
        //duration = "\(periodState.duration)"
        //interval = "\(periodState.interval)"

        return VStack {
            Image(systemName: "globe")
                .imageScale(.large)
                .foregroundStyle(.tint)
            HStack {
                Text("Count: \(count)")
                Button("Increment") {
                    count += 1;
                    UserDefaults(suiteName: "group.com.natalianaumova.ona")!.set(count, forKey: "Count");
                }
            }
            //TextField("Start", text: $start)
            //TextField("Duration", text: $duration)
            //TextField("Interval", text: $interval)
        }
        .padding()
    }
    
    func getPeriodState() -> PeriodState {
        let fallback = PeriodState(start: "N/A", duration: 0, interval: 0)
        
        guard let userDefaults = UserDefaults(suiteName: "group.com.natalianaumova.ona") else {
            return fallback
        }
        return userDefaults.object(forKey: "PeriodState") as? PeriodState ?? fallback
    }
}

struct PeriodState {
    let start: String
    let duration: Int
    let interval: Int
}

#Preview {
    ContentView()
}
