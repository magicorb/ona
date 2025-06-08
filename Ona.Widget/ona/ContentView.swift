//
//  ContentView.swift
//  ona
//
//  Created by Natalia Naumova on 29/07/2024.
//

import SwiftUI

struct ContentView: View {
    @State private var count = UserDefaults(suiteName: "group.com.natalianaumova.ona")!.integer(forKey: "Count");
    var body: some View {
        VStack {
            Image(systemName: "globe")
                .imageScale(.large)
                .foregroundStyle(.tint)
            Text("Count: \(count)")
            Button("Increment") {
                count += 1;
                UserDefaults(suiteName: "group.com.natalianaumova.ona")!.set(count, forKey: "Count");
            }
        }
        .padding()
    }
}

#Preview {
    ContentView()
}
