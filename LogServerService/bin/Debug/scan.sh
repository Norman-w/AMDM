#!/bin/bash
# Ping �^��W·�����е� IP λַ
for ip in 140.110.99.{1..254}; do
  # �h���f�� arp ӛ�
  sudo arp -d $ip > /dev/null 2>&1
  # ���� ping ȡ���µ� arp �YӍ
  ping -c 5 $ip > /dev/null 2>&1 &
done

# �ȴ����б����� Ping �Y��
wait

# ݔ�� ARP table
arp -na | grep -v incomplete