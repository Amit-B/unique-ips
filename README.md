# unique-ips

The story of this one is quite funny.

Somewhere around 2016 my gaming community servers were attacked with some sort of DDoS attacks. Using around 20~ attacked IP addresses, out of hundreds and thousands of real player IPs, I had to find the attacker IP addresses.
I thought it would be easy making a program to find the addresses that appear the most, considering they're the attacker IPs.

The only problem was that in only a few seconds, the security logs were spammed with more than 200K enteries! Which caused my program hours of searching. Therefore, I was researching for solution. Using multi-threading techniques I figured out a way to scan these 200K+ enteries in less than 10 seconds.

The program helped the community a lot, outputing IP addresses of attackers with firewall block syntax, making it VERY easy to block them.
