%% Loading measurement files
phase_2442018 = load('phase_2442018.txt');
temperature_2442018 = load('temperature_2442018.txt');
phase_2542018 = load('phase_2542018.txt');
temperature_2542018 = load('temperature_2542018.txt');
dac_value_2542018 = load('dac_value_2542018.txt');
%% Plotting phase difference for hardware with potenciometer
figure;
plot(phase_2442018, 'LineWidth', 1.5);
axis([0 length(phase_2442018) min(phase_2442018)-10e-8 max(phase_2442018)+10e-7]);
hold on;
grid on;
xlabel('Time [s]');
ylabel('Phase difference [s]');
title('Phase difference for hardware with potenciometer');
hold off;
%% Plotting delta phase = phase(n) - phase(n-1), where 'n' is number of seconds (for hardware with potenciometer)
delta_phase_2442018 = phase_2442018(1:length(phase_2442018)-1) - phase_2442018(2:length(phase_2442018));
figure;
plot(delta_phase_2442018);
axis([0 length(delta_phase_2442018) min(delta_phase_2442018) max(delta_phase_2442018)]);
hold on;
mean_val = mean(delta_phase_2442018);
a = [0, mean_val;
    length(delta_phase_2442018), mean_val];
plot(a(:,1), a(:,2), 'LineWidth', 1.5);
grid on;
xlabel('Time [s]');
ylabel('\Delta phase [s]');
legend('Delta phase', strcat('Mean value = ', num2str(mean_val)));
title('\Delta phase for hardware with potenciometer');
hold off;
%% Plotting temperature for hardware with potenciometer
figure;
stairs(temperature_2442018(:,1), temperature_2442018(:,2), 'LineWidth', 1.5);
axis([0 max(temperature_2442018(:,1)) 25 max(temperature_2442018(:,2))+1]);
hold on;
grid on;
xlabel('Time [s]');
ylabel('Temperature [°C]');
title('Temperature for hardware with potenciometer');
hold off;
%% Plotting phase difference for hardware with DAC
figure;
plot(phase_2542018, 'LineWidth', 1.5);
hold on;
grid on;
axis([0 length(phase_2542018) -4.5e-5 6e-7]);
xlabel('Time [s]');
ylabel('Phase difference [s]');
title('Phase difference for hardware with DAC');
hold off;
%% Plotting delta phase for hardware with DAC
% Removing outliers
tmp_phase = [];
for i = 1:length(phase_2542018)
    if(abs(phase_2542018(i)) < 1e-3)
        tmp_phase = [tmp_phase phase_2542018(i)];
    end
end
delta_phase_2542018 = tmp_phase(1:length(tmp_phase)-1) - tmp_phase(2:length(tmp_phase));
figure;
plot(delta_phase_2542018);
axis([0 length(delta_phase_2542018) -1.2e-7 1.2e-7]);
hold on;
mean_val2 = mean(delta_phase_2542018);
a2 = [0, mean_val2;
    length(delta_phase_2542018), mean_val2];
plot(a2(:,1), a2(:,2), 'LineWidth', 1.5);
grid on;
xlabel('Time [s]');
ylabel('\Delta phase [s]');
legend('Delta phase', strcat('Mean value = ', num2str(mean_val2)));
title('\Delta phase for hardware with DAC');
hold off;
%% Plotting temperature for hardware with DAC
figure;
stairs(temperature_2542018(:,1), temperature_2542018(:,2), 'LineWidth', 1.5);
axis([0 max(temperature_2542018(:,1)) 25 max(temperature_2542018(:,2))+1]);
hold on;
grid on;
xlabel('Time [s]');
ylabel('Temperature [°C]');
title('Temperature for hardware with DAC');
hold off;
%% Plotting DAC Values for hardware with DAC
figure;
plot(dac_value_2542018(:,1), dac_value_2542018(:,2), 'LineWidth', 1.5);
axis([0 max(dac_value_2542018(:,1)) 135000 139000]);
hold on;
grid on;
xlabel('Time [s]');
ylabel('DAC Value');
title('DAC Values for hardware with DAC (last dac value 135870)');
hold off;
