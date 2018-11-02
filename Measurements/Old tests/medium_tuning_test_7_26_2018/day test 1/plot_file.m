phase = load('phase_7_26_2018.txt');
figure;
x = [1669:1:length(phase)];
plot(x, phase(1669:end))
axis([1669 length(phase) -2e-8 2e-8])
hold on;
grid on
xlabel('Time [s]');
ylabel('Phase Difference [s]');
title('Fine tuning only');

figure;
x = [609:1:1669];
plot(x,phase(609:1669))
axis([609 1669 min(phase(609:1669)) max(phase(609:1669))])
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

startIndex = 609
stopIndex = 1669
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

