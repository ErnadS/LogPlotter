phase = load('phase_7_26_2018.txt');
figure;
x = [1800:1:length(phase)];
plot(x, phase(1800:end))
axis([1800 length(phase) -8e-8 8e-8])
hold on;
grid on
xlabel('Time [s]');
ylabel('Phase Difference [s]');
title('Fine tuning only');

figure;
x = [600:1:1800];
plot(x,phase(600:1800))
axis([600 1800 min(phase(600:1800)) max(phase(600:1800))])
hold on;
grid on
xlabel('Time [s]');
ylabel('Phase Difference [s]');
title('Medium tuning only');


figure;
plot(phase)
axis([0 length(phase) min(phase) max(phase)])
hold on;
grid on
xlabel('Time [s]');
ylabel('Phase Difference [s]');
title('Whole graph');
%%
dac = load('dac_value_7_26_2018.txt');
time = dac(:,1);
value = dac(:,2);
figure
plot(time, value)
axis([0 max(time) min(value)-100 max(value)+1000])
hold on
grid on
xlabel('Time [s]');
ylabel('DAC Value');
title('Whole graph');

startIndex = find(time == 6.301049519000000e+02)
stopIndex = find(time == 1.730084958700000e+03)
figure
plot(time(startIndex:stopIndex), value(startIndex:stopIndex))
axis([time(startIndex) time(stopIndex) min(value(startIndex:stopIndex))-10 max(value(startIndex:stopIndex))+10]);
hold on;
grid on;
xlabel('Time [s]');
ylabel('DAC Value');
title('Medium tuning only');

figure
stairs(time(stopIndex:end), value(stopIndex:end))
axis([time(stopIndex) time(end) min(value(stopIndex:end))-1 max(value(stopIndex:end))+1])
hold on
grid on
xlabel('Time [s]');
ylabel('DAC Value');
title('Fine tuning only');

